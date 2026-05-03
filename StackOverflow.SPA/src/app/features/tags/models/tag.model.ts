export interface TagResponse {
  name: string;
  count: number;
  percentage: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}
