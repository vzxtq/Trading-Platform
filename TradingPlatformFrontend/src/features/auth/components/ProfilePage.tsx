import { useState } from 'react'
import { ProfileSidebar } from './ProfileSidebar'
import { ProfileOverview } from './ProfileOverview'
import { UpdateProfileForm } from './UpdateProfileForm'
import { UpdatePasswordForm } from './UpdatePasswordForm'

export const ProfilePage = () => {
  const [activeTab, setActiveTab] = useState<'overview' | 'update'>('overview')

  return (
    <div className="flex h-screen bg-background text-foreground overflow-hidden font-sans">
      <ProfileSidebar activeTab={activeTab} onTabChange={setActiveTab} />

      <main className="flex-1 overflow-y-auto bg-background px-12 py-12">
        <div className="max-w-[1000px]">
          {activeTab === 'overview' ? (
            <ProfileOverview />
          ) : (
            <div className="max-w-[800px]">
              <h1 className="text-3xl font-bold text-foreground mb-2">Update account</h1>
              <p className="text-base text-muted-foreground mb-12 font-medium tracking-tight">
                Manage your personal information and security settings
              </p>
              <UpdateProfileForm />
              <UpdatePasswordForm />
            </div>
          )}
        </div>
      </main>
    </div>
  )
}
