import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { toast } from 'sonner'
import { UpdatePasswordSchema, type UpdatePasswordRequest } from '../types/auth-requests.types'
import { useUpdatePassword } from '../api/auth.api'
import { handleApiError } from '@/lib/error-handler'

export const usePasswordForm = () => {
  const { mutate: updatePassword } = useUpdatePassword()

  const form = useForm<UpdatePasswordRequest>({
    resolver: zodResolver(UpdatePasswordSchema),
    defaultValues: {
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    },
  })

  const onSubmit = (data: UpdatePasswordRequest) => {
    updatePassword(
      { currentPassword: data.currentPassword, newPassword: data.newPassword },
      {
        onSuccess: () => {
          toast.success('Password updated successfully')
          form.reset()
        },
        onError: (err) => handleApiError(err),
      }
    )
  }

  return { form, onSubmit }
}
