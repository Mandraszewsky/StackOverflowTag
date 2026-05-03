export interface TagQuery {
  pageNumber: number;
  pageSize: number;
  sortBy: string;
  sortDirection: 'asc' | 'desc';
}

export const defaultTagQuery: TagQuery = {
  pageNumber: 1,
  pageSize: 25,
  sortBy: '',
  sortDirection: 'asc',
};
