import React from 'react'
import { useAuth } from '../hooks/useAuth'
import { useAccount, usePositions } from '../api/auth.api'
import { Badge } from '@/components/ui/badge'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { 
  User, 
  LayoutDashboard, 
  Layers, 
  History, 
  Settings, 
  LogOut,
  Wallet,
  Lock,
  TrendingUp
} from 'lucide-react'

const SidebarItem: React.FC<{ 
  icon: React.ReactNode, 
  label: string, 
  active?: boolean, 
  onClick?: () => void,
  danger?: boolean 
}> = ({ icon, label, active, onClick, danger }) => (
  <button
    onClick={onClick}
    className={`flex w-full items-center space-x-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
      active 
        ? 'bg-[#262626] text-white' 
        : danger 
          ? 'text-red-400 hover:bg-red-950/30' 
          : 'text-neutral-400 hover:bg-[#262626] hover:text-white'
    }`}
  >
    {icon}
    <span>{label}</span>
  </button>
)

export const ProfilePage: React.FC = () => {
  const { userId, logout } = useAuth()
  const { data: account, isLoading: isAccountLoading } = useAccount(userId)
  const { data: positions, isLoading: isPositionsLoading } = usePositions()

  if (isAccountLoading || isPositionsLoading) {
    return <div className="flex min-h-screen items-center justify-center bg-black text-white">Loading profile...</div>
  }

  if (!account) {
    return <div className="flex min-h-screen items-center justify-center bg-black text-white">Account not found</div>
  }

  const totalPnL = positions?.reduce((acc, pos) => acc + (pos.unrealizedPnL || 0), 0) || 0
  const pnlColor = totalPnL >= 0 ? 'text-green-500' : 'text-red-500'
  const pnlPrefix = totalPnL >= 0 ? '+' : ''

  const initials = account.name.split(' ').map(n => n[0]).join('')

  const currencyMap: Record<number, string> = {
    0: 'USD',
    1: 'EUR',
    2: 'GBP',
    3: 'JPY'
  }

  const formatCurrency = (amount: number, currency: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currencyMap[currency] || 'USD'
    }).format(amount)
  }

  const formatDate = (dateStr: string | null) => {
    if (!dateStr) return 'N/A'
    return new Date(dateStr).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    })
  }

  return (
    <div className="flex min-h-screen bg-black text-white">
      {/* Sidebar */}
      <aside className="w-64 border-r border-neutral-800 bg-[#121212] p-6">
        <div className="mb-8 flex flex-col items-center space-y-3 border-b border-neutral-800 pb-8 text-center">
          <div className="flex h-16 w-16 items-center justify-center rounded-full bg-[#262626] text-xl font-bold">
            {initials}
          </div>
          <div>
            <h2 className="text-lg font-bold">{account.name}</h2>
            <p className="text-sm text-neutral-400">{account.email}</p>
            <Badge variant="secondary" className="mt-2 bg-green-950/30 text-green-500 border-green-900/50">
              Active
            </Badge>
          </div>
        </div>

        <nav className="space-y-1">
          <SidebarItem icon={<User size={18} />} label="Profile" active />
          <SidebarItem icon={<LayoutDashboard size={18} />} label="My orders" />
          <SidebarItem icon={<Layers size={18} />} label="Positions" />
          <SidebarItem icon={<History size={18} />} label="Trade history" />
          <SidebarItem icon={<Settings size={18} />} label="Settings" />
          <div className="pt-4">
            <SidebarItem icon={<LogOut size={18} />} label="Sign out" danger onClick={logout} />
          </div>
        </nav>
      </aside>

      {/* Main Content */}
      <main className="flex-1 overflow-auto p-8">
        <h1 className="mb-6 text-2xl font-bold">Overview</h1>
        
        <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
          <Card className="border-neutral-800 bg-[#1a1a1a] text-white">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-neutral-400">Balance</CardTitle>
              <Wallet className="h-4 w-4 text-neutral-400" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">
                {formatCurrency(account.balance.amount, account.balance.currency)}
              </div>
            </CardContent>
          </Card>

          <Card className="border-neutral-800 bg-[#1a1a1a] text-white">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-neutral-400">Reserved</CardTitle>
              <Lock className="h-4 w-4 text-neutral-400" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">
                {formatCurrency(account.reservedBalance.amount, account.reservedBalance.currency)}
              </div>
            </CardContent>
          </Card>

          <Card className="border-neutral-800 bg-[#1a1a1a] text-white">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-neutral-400">Positions</CardTitle>
              <Layers className="h-4 w-4 text-neutral-400" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{positions?.length || 0}</div>
            </CardContent>
          </Card>

          <Card className="border-neutral-800 bg-[#1a1a1a] text-white">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-neutral-400">Total P&L</CardTitle>
              <TrendingUp className={`h-4 w-4 ${pnlColor}`} />
            </CardHeader>
            <CardContent>
              <div className={`text-2xl font-bold ${pnlColor}`}>
                {pnlPrefix}{formatCurrency(totalPnL, account.balance.currency)}
              </div>
            </CardContent>
          </Card>
        </div>

        <h2 className="mb-4 mt-12 text-2xl font-bold">Account details</h2>
        <Card className="border-neutral-800 bg-[#1a1a1a] text-white overflow-hidden">
          <div className="divide-y divide-neutral-800">
            <div className="flex items-center justify-between p-4">
              <span className="text-neutral-400">Full name</span>
              <span className="font-medium">{account.name}</span>
            </div>
            <div className="flex items-center justify-between p-4">
              <span className="text-neutral-400">Email</span>
              <span className="font-medium">{account.email}</span>
            </div>
            <div className="flex items-center justify-between p-4">
              <span className="text-neutral-400">Currency</span>
              <span className="font-medium">{currencyMap[account.balance.currency]}</span>
            </div>
            <div className="flex items-center justify-between p-4">
              <span className="text-neutral-400">Member since</span>
              <span className="font-medium">{formatDate(account.createdAt)}</span>
            </div>
            <div className="flex items-center justify-between p-4">
              <span className="text-neutral-400">Last login</span>
              <span className="font-medium">{formatDate(account.lastLoginAt)}</span>
            </div>
          </div>
        </Card>
      </main>
    </div>
  )
}
