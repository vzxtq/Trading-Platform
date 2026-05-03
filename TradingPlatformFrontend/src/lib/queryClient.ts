import { QueryClient, QueryCache, MutationCache } from '@tanstack/react-query'
import { handleApiError } from './error-handler'

export const queryClient = new QueryClient({
  queryCache: new QueryCache({
    onError: (error) => {
      handleApiError(error)
    },
  }),
  mutationCache: new MutationCache({
    onError: (error) => {
      handleApiError(error)
    },
  }),
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: 2,
      refetchOnWindowFocus: false,
    },
    mutations: {
      retry: 0,
    },
  },
})

