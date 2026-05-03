import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult, TagResponse } from './models/tag.model';
import { TagQuery } from './models/tag-query.model';

@Injectable({ providedIn: 'root' })
export class TagApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/tags`;

  getTags(query: TagQuery): Observable<PagedResult<TagResponse>> {
    const params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize)
      .set('sortBy', query.sortBy)
      .set('sortDirection', query.sortDirection);

    return this.http.get<PagedResult<TagResponse>>(this.baseUrl, { params });
  }

  refreshTags(): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/refresh`, {});
  }
}
