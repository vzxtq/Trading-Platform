import { toast } from "sonner"
import axios from "axios"

export const handleApiError = (error: unknown) => {
  if (axios.isAxiosError(error)) {
    const status = error.response?.status

    // 5xx — never expose backend internals to the user.
    if (status && status >= 500) {
      toast.error("Something went wrong", {
        description: "Please try again later.",
      })
      return
    }

    const data = error.response?.data as any

    // 4xx with our standard ApiResponse structure — these are intentional
    // business errors the backend explicitly chose to surface.
    if (data?.message || (data?.errors && data.errors.length > 0)) {
      const message = data.message || "Action failed"
      const details = Array.isArray(data.errors)
        ? data.errors.join(". ")
        : typeof data.errors === "string"
        ? data.errors
        : undefined

      toast.error(message, { description: details })
      return
    }

    // Handle standard Problem Details (RFC 7807) used by .NET
    if (data?.title || data?.detail) {
      toast.error(data.title || "Error", {
        description: data.detail || (data.errors ? JSON.stringify(data.errors) : undefined),
      })
      return
    }

    switch (status) {
      case 400:
        toast.error("Invalid Request", { description: "The server could not understand the request." })
        break
      case 401:
        // Silently handled by auth interceptor
        break
      case 402:
        toast.error("Insufficient Funds", { description: "You don't have enough balance for this operation." })
        break
      case 403:
        toast.error("Access Denied", { description: "You don't have permission to perform this action." })
        break
      case 404:
        toast.error("Not Found", { description: "The requested resource was not found." })
        break
      case 415:
        toast.error("Unsupported Format", { description: "The request data format is not supported." })
        break
      case 429:
        toast.error("Too Many Requests", { description: "Please slow down and try again later." })
        break
      default:
        toast.error("Network Error", { description: "Please check your connection or try again later." })
    }
  } else {
    // Non-Axios errors (JS exceptions, etc.) — never expose internal messages.
    toast.error("Something went wrong", {
      description: "Please try again later.",
    })
  }
}
