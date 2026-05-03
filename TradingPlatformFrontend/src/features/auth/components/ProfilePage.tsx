import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { toast } from 'sonner'
import {
  UpdateProfileSchema,
  type UpdateProfileRequest,
  UpdatePasswordSchema,
  type UpdatePasswordRequest,
} from '../types/auth-requests.types'
import { useAccount, useUpdateProfile, useUpdatePassword } from '../api/auth.api'
import { useAuthStore } from '@/store/auth'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { 
  User, 
  Shield, 
  FileText, 
  BarChart2, 
  History, 
  LogOut,
  ArrowUpRight,
  UserCircle,
  UserCog
} from 'lucide-react'
import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { cn, formatCurrency } from '@/lib/utils'

export const ProfilePage = () => {
  const userId = useAuthStore((state) => state.userId)
  const clearAuth = useAuthStore((state) => state.clearAuth)
  const { data: account } = useAccount(userId)
  const { mutate: updateProfile } = useUpdateProfile()
  const { mutate: updatePassword } = useUpdatePassword()
  const navigate = useNavigate()
  
  const [activeTab, setActiveTab] = useState<'overview' | 'update'>('overview')

  const profileForm = useForm<UpdateProfileRequest>({
    resolver: zodResolver(UpdateProfileSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
    },
  })

  const passwordForm = useForm<UpdatePasswordRequest>({
    resolver: zodResolver(UpdatePasswordSchema),
    defaultValues: {
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    },
  })

  useEffect(() => {
    if (account) {
      const names = account.name.split(' ')
      profileForm.reset({
        firstName: names[0] || '',
        lastName: names.slice(1).join(' ') || '',
        email: account.email,
      })
    }
  }, [account, profileForm])

  const onProfileSubmit = (data: UpdateProfileRequest) => {
    updateProfile(data, {
      onSuccess: () => toast.success('Profile updated successfully'),
      onError: (err: any) => toast.error(err.message || 'Failed to update profile'),
    })
  }

  const onPasswordSubmit = (data: UpdatePasswordRequest) => {
    updatePassword(
      { currentPassword: data.currentPassword, newPassword: data.newPassword },
      {
        onSuccess: () => {
          toast.success('Password updated successfully')
          passwordForm.reset()
        },
        onError: (err: any) => toast.error(err.message || 'Failed to update password'),
      }
    )
  }

  const handleLogout = () => {
    clearAuth()
    navigate('/login')
  }

  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2)
  }

  return (
    <div className="flex h-screen bg-background text-foreground overflow-hidden font-sans">
      {/* Sidebar */}
      <aside className="w-[280px] border-r border-border bg-background flex flex-col shrink-0">
        <div className="p-6">
          <div className="flex items-center justify-between mb-10">
            <span className="text-sm font-bold tracking-wider text-foreground uppercase cursor-pointer" onClick={() => navigate('/dashboard')}>Trading Engine</span>
            <ArrowUpRight size={16} className="text-muted-foreground" />
          </div>

          <div className="mb-10">
            <div className="w-16 h-16 rounded-full bg-muted flex items-center justify-center text-xl font-medium text-muted-foreground mb-4">
              {account ? getInitials(account.name) : '??'}
            </div>
            <h2 className="text-lg font-bold text-foreground leading-tight mb-1">{account?.name || 'User'}</h2>
            <p className="text-sm text-muted-foreground mb-3">{account?.email}</p>
            <div className="inline-block px-2 py-0.5 border border-green-500/20 rounded text-[10px] font-bold text-green-500 uppercase tracking-wider bg-green-500/10">
              Active
            </div>
          </div>

          <nav className="space-y-1">
            <button
              onClick={() => setActiveTab('overview')}
              className={cn(
                "w-full flex items-center gap-3 px-4 py-3 text-sm font-medium transition-colors relative group",
                activeTab === 'overview' ? "text-foreground bg-muted/50" : "text-muted-foreground hover:text-foreground"
              )}
            >
              {activeTab === 'overview' && <div className="absolute left-0 top-0 bottom-0 w-1 bg-green-500" />}
              <UserCircle size={18} className={cn(activeTab === 'overview' ? "text-foreground" : "text-muted-foreground group-hover:text-foreground")} />
              Profile
            </button>
            <button className="w-full flex items-center gap-3 px-4 py-3 text-sm font-medium text-muted-foreground hover:text-foreground transition-colors"><FileText size={18} />My orders</button>
            <button className="w-full flex items-center gap-3 px-4 py-3 text-sm font-medium text-muted-foreground hover:text-foreground transition-colors"><BarChart2 size={18} />Positions</button>
            <button className="w-full flex items-center gap-3 px-4 py-3 text-sm font-medium text-muted-foreground hover:text-foreground transition-colors"><History size={18} />Trade history</button>
            <button
              onClick={() => setActiveTab('update')}
              className={cn(
                "w-full flex items-center gap-3 px-4 py-3 text-sm font-medium transition-colors relative group",
                activeTab === 'update' ? "text-foreground bg-muted/50" : "text-muted-foreground hover:text-foreground"
              )}
            >
              {activeTab === 'update' && <div className="absolute left-0 top-0 bottom-0 w-1 bg-green-500" />}
              <UserCog size={18} className={cn(activeTab === 'update' ? "text-foreground" : "text-muted-foreground group-hover:text-foreground")} />
              Update account
            </button>
          </nav>
        </div>

        <div className="mt-auto p-6">
          <button 
            onClick={handleLogout}
            className="flex items-center gap-3 text-muted-foreground hover:text-destructive transition-colors text-sm font-medium"
          >
            <LogOut size={18} />
            <span className="text-destructive">Sign out</span>
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <main className="flex-1 overflow-y-auto bg-background px-12 py-12">
        <div className="max-w-[1000px]">
          {activeTab === 'overview' ? (
            <>
              <h1 className="text-3xl font-bold text-foreground mb-8">Overview</h1>
              <div className="grid grid-cols-2 gap-6 mb-12">
                <div className="bg-card p-6 rounded-xl border border-border">
                  <span className="text-[10px] font-bold uppercase tracking-widest text-muted-foreground block mb-4">Balance</span>
                  <div className="flex items-baseline gap-2">
                    <span className="text-3xl font-bold text-foreground">{account ? formatCurrency(account.balance.amount, account.balance.currency).split(' ')[0] : '0.00'}</span>
                    <span className="text-sm font-bold text-muted-foreground">{account?.balance.currency}</span>
                  </div>
                </div>
                <div className="bg-card p-6 rounded-xl border border-border">
                  <span className="text-[10px] font-bold uppercase tracking-widest text-muted-foreground block mb-4">Reserved</span>
                  <div className="flex items-baseline gap-2">
                    <span className="text-3xl font-bold text-[#f59e0b]">{account ? formatCurrency(account.reservedBalance.amount, account.reservedBalance.currency).split(' ')[0] : '0.00'}</span>
                    <span className="text-sm font-bold text-muted-foreground">{account?.reservedBalance.currency}</span>
                  </div>
                </div>
                <div className="bg-card p-6 rounded-xl border border-border">
                  <span className="text-[10px] font-bold uppercase tracking-widest text-muted-foreground block mb-4">Available</span>
                  <div className="flex items-baseline gap-2">
                    <span className="text-3xl font-bold text-green-500">{account ? formatCurrency(account.availableBalance.amount, account.availableBalance.currency).split(' ')[0] : '0.00'}</span>
                    <span className="text-sm font-bold text-muted-foreground">{account?.availableBalance.currency}</span>
                  </div>
                </div>
                <div className="bg-card p-6 rounded-xl border border-border">
                  <span className="text-[10px] font-bold uppercase tracking-widest text-muted-foreground block mb-4">Total P&L</span>
                  <div className="flex items-baseline gap-2">
                    <span className="text-3xl font-bold text-green-500">+0.00</span>
                    <span className="text-sm font-bold text-muted-foreground">{account?.balance.currency}</span>
                  </div>
                </div>
              </div>
              <h2 className="text-2xl font-bold text-foreground mb-6">Account details</h2>
              <div className="bg-card rounded-xl border border-border overflow-hidden">
                <div className="flex items-center justify-between p-6 border-b border-border"><span className="text-sm font-medium text-muted-foreground">Full name</span><span className="text-sm font-bold text-foreground">{account?.name}</span></div>
                <div className="flex items-center justify-between p-6 border-b border-border"><span className="text-sm font-medium text-muted-foreground">Email</span><span className="text-sm font-bold text-foreground">{account?.email}</span></div>
                <div className="flex items-center justify-between p-6 border-b border-border"><span className="text-sm font-medium text-muted-foreground">Member since</span><span className="text-sm font-bold text-foreground">{account ? new Date(account.createdAt).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' }) : 'N/A'}</span></div>
                <div className="flex items-center justify-between p-6"><span className="text-sm font-medium text-muted-foreground">Last login</span><span className="text-sm font-bold text-foreground">{account?.lastLoginAt ? new Date(account.lastLoginAt).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' }) : 'N/A'}</span></div>
              </div>
            </>
          ) : (
            <div className="max-w-[800px]">
              <h1 className="text-3xl font-bold text-foreground mb-2">Update account</h1>
              <p className="text-base text-muted-foreground mb-12 font-medium tracking-tight">Manage your personal information and security settings</p>
              <form onSubmit={profileForm.handleSubmit(onProfileSubmit)} className="space-y-8 mb-16">
                <div className="flex items-center gap-3 mb-8"><User size={18} className="text-muted-foreground" /><span className="text-xs font-bold uppercase tracking-[0.15em] text-muted-foreground whitespace-nowrap">Personal Information</span><div className="h-[1px] w-full bg-border" /></div>
                <div className="grid grid-cols-2 gap-x-6 gap-y-6">
                  <div className="space-y-2"><Label className="text-sm text-muted-foreground font-medium">First name</Label><Input {...profileForm.register('firstName')} className="bg-card border-border text-foreground h-[52px] px-4 text-base focus-visible:ring-1 focus-visible:ring-green-500/30 placeholder:text-muted-foreground/30" /></div>
                  <div className="space-y-2"><Label className="text-sm text-muted-foreground font-medium">Last name</Label><Input {...profileForm.register('lastName')} className="bg-card border-border text-foreground h-[52px] px-4 text-base focus-visible:ring-1 focus-visible:ring-green-500/30 placeholder:text-muted-foreground/30" /></div>
                  <div className="col-span-2 space-y-2"><Label className="text-sm text-muted-foreground font-medium">Email address</Label><Input {...profileForm.register('email')} type="email" className="bg-card border-border text-foreground h-[52px] px-4 text-base focus-visible:ring-1 focus-visible:ring-green-500/30 placeholder:text-muted-foreground/30" /></div>
                </div>
                <div className="flex justify-end"><Button type="submit" className="bg-green-500 hover:bg-green-600 text-black font-bold px-10 h-[48px] text-base rounded-lg transition-all active:scale-[0.98]">Save changes</Button></div>
              </form>
              <form onSubmit={passwordForm.handleSubmit(onPasswordSubmit)} className="space-y-8">
                <div className="flex items-center gap-3 mb-8"><Shield size={18} className="text-muted-foreground" /><span className="text-xs font-bold uppercase tracking-[0.15em] text-muted-foreground whitespace-nowrap">Security</span><div className="h-[1px] w-full bg-border" /></div>
                <div className="space-y-6">
                  <div className="space-y-2"><Label className="text-sm text-muted-foreground font-medium">Current password</Label><Input {...passwordForm.register('currentPassword')} type="password" className="bg-card border-border text-foreground h-[52px] px-4 text-base focus-visible:ring-1 focus-visible:ring-muted-foreground/30 placeholder:text-muted-foreground/30" /></div>
                  <div className="grid grid-cols-2 gap-x-6 gap-y-6">
                    <div className="space-y-2"><Label className="text-sm text-muted-foreground font-medium">New password</Label><Input {...passwordForm.register('newPassword')} type="password" className="bg-card border-border text-foreground h-[52px] px-4 text-base focus-visible:ring-1 focus-visible:ring-muted-foreground/30 placeholder:text-muted-foreground/30" /></div>
                    <div className="space-y-2"><Label className="text-sm text-muted-foreground font-medium">Confirm new password</Label><Input {...passwordForm.register('confirmPassword')} type="password" className="bg-card border-border text-foreground h-[52px] px-4 text-base focus-visible:ring-1 focus-visible:ring-muted-foreground/30 placeholder:text-muted-foreground/30" />{passwordForm.formState.errors.confirmPassword && <p className="text-destructive text-xs font-bold mt-2 uppercase tracking-tight">{passwordForm.formState.errors.confirmPassword.message}</p>}</div>
                  </div>
                </div>
                <div className="flex justify-end"><Button type="submit" variant="outline" className="border border-border bg-transparent text-foreground font-bold px-10 h-[48px] text-base rounded-lg hover:bg-muted transition-all active:scale-[0.98]">Update password</Button></div>
              </form>
            </div>
          )}
        </div>
      </main>
    </div>
  )
}
