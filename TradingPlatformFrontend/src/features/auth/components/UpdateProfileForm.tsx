import { User } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { useProfileForm } from '../hooks/useProfileForm'

export const UpdateProfileForm = () => {
  const { form, onSubmit } = useProfileForm()

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8 mb-16">
      <div className="flex items-center gap-3 mb-8">
        <User size={18} className="text-muted-foreground" />
        <span className="text-xs font-bold uppercase tracking-[0.15em] text-muted-foreground whitespace-nowrap">
          Personal Information
        </span>
        <div className="h-[1px] w-full bg-border" />
      </div>
      <div className="grid grid-cols-2 gap-x-6 gap-y-6">
        <div className="space-y-2">
          <Label className="text-sm text-muted-foreground font-medium">First name</Label>
          <Input 
            {...form.register('firstName')} 
            className="bg-card border-border text-foreground h-[52px] px-4 text-base focus-visible:ring-1 focus-visible:ring-green-500/30 placeholder:text-muted-foreground/30" 
          />
        </div>
        <div className="space-y-2">
          <Label className="text-sm text-muted-foreground font-medium">Last name</Label>
          <Input 
            {...form.register('lastName')} 
            className="bg-card border-border text-foreground h-[52px] px-4 text-base focus-visible:ring-1 focus-visible:ring-green-500/30 placeholder:text-muted-foreground/30" 
          />
        </div>
        <div className="col-span-2 space-y-2">
          <Label className="text-sm text-muted-foreground font-medium">Email address</Label>
          <Input 
            {...form.register('email')} 
            type="email" 
            className="bg-card border-border text-foreground h-[52px] px-4 text-base focus-visible:ring-1 focus-visible:ring-green-500/30 placeholder:text-muted-foreground/30" 
          />
        </div>
      </div>
      <div className="flex justify-end">
        <Button 
          type="submit" 
          className="bg-green-500 hover:bg-green-600 text-black font-bold px-10 h-[48px] text-base rounded-lg transition-all active:scale-[0.98]"
        >
          Save changes
        </Button>
      </div>
    </form>
  )
}
