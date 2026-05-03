import { computed, inject } from '@angular/core';
import {
  patchState,
  signalStore,
  withComputed,
  withHooks,
  withMethods,
  withState,
} from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { TagApiService } from './tag-api.service';
import { NotifyService } from '../../core/services/notify.service';
import { TagResponse } from './models/tag.model';
import { defaultTagQuery, TagQuery } from './models/tag-query.model';

interface TagState {
  tags: TagResponse[];
  totalCount: number;
  loading: boolean;
  query: TagQuery;
}

const initialState: TagState = {
  tags: [],
  totalCount: 0,
  loading: false,
  query: { ...defaultTagQuery },
};

export const TagStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((store) => ({
    currentPage: computed(() => store.query().pageNumber),
    pageSize: computed(() => store.query().pageSize),
    sortBy: computed(() => store.query().sortBy),
    sortDirection: computed(() => store.query().sortDirection),
  })),
  withMethods(
    (store, api = inject(TagApiService), notify = inject(NotifyService)) => ({
      loadTags: rxMethod<void>(
        pipe(
          tap(() => patchState(store, { loading: true })),
          switchMap(() =>
            api.getTags(store.query()).pipe(
              tapResponse({
                next: (result) =>
                  patchState(store, {
                    tags: result.items,
                    totalCount: result.totalCount,
                    loading: false,
                  }),
                error: () => {
                  patchState(store, { loading: false });
                  notify.error('Failed to load tags');
                },
              })
            )
          )
        )
      ),
      refreshTags: rxMethod<void>(
        pipe(
          tap(() => patchState(store, { loading: true })),
          switchMap(() =>
            api.refreshTags().pipe(
              tapResponse({
                next: () => notify.success('Tags refreshed successfully'),
                error: () => {
                  patchState(store, { loading: false });
                  notify.error('Failed to refresh tags');
                },
              })
            )
          ),
          switchMap(() =>
            api.getTags(store.query()).pipe(
              tapResponse({
                next: (result) =>
                  patchState(store, {
                    tags: result.items,
                    totalCount: result.totalCount,
                    loading: false,
                  }),
                error: () => {
                  patchState(store, { loading: false });
                  notify.error('Failed to load tags after refresh');
                },
              })
            )
          )
        )
      ),
      changePage(pageNumber: number, pageSize: number): void {
        patchState(store, (state) => ({
          query: { ...state.query, pageNumber, pageSize },
        }));
        this.loadTags();
      },
      changeSort(sortBy: string, sortDirection: 'asc' | 'desc'): void {
        patchState(store, (state) => ({
          query: { ...state.query, sortBy, sortDirection, pageNumber: 1 },
        }));
        this.loadTags();
      },
    })
  ),
  withHooks({
    onInit(store) {
      store.loadTags();
    },
  })
);
