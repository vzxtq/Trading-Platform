import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { useEffect } from 'react'
import { toast } from 'sonner'
import { UpdateProfileSchema, type UpdateProfileRequest } from '../types/auth-requests.types'
import { useAccount, useUpdateProfile } from '../api/auth.api'
import { useAuthStore } from '@/store/auth'
import { handleApiError } from '@/lib/error-handler'

export const useProfileForm = () => {
  const userId = useAuthStore((state) => state.userId)
  const { data: account } = useAccount(userId)
  const { mutate: updateProfile } = useUpdateProfile()

  const form = useForm<UpdateProfileRequest>({
    resolver: zodResolver(UpdateProfileSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
    },
  })

  useEffect(() => {
    if (account) {
      form.reset({
        firstName: account.firstName,
        lastName: account.lastName,
        email: account.email,
      })
    }
  }, [account, form])

  const onSubmit = (data: UpdateProfileRequest) => {
    updateProfile(data, {
      onSuccess: () => toast.success('Profile updated successfully'),
      onError: (err) => handleApiError(err),
    })
  }

  return { form, onSubmit }
}
