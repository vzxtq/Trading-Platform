import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr'
import { HUB_PATHS } from '@/lib/constants'
import { useAuthStore } from '@/store/auth'

function getRequiredEnv(name: string): string {
  const value = import.meta.env[name as keyof ImportMetaEnv]
  if (!value) {
    throw new Error(`Missing required env var: ${name}`)
  }
  return value
}

type HubKind = keyof typeof HUB_PATHS

class BaseHubConnection {
  private readonly kind: HubKind
  protected connection: HubConnection | null = null

  constructor(kind: HubKind) {
    this.kind = kind
  }

  protected buildConnection(): HubConnection {
    const baseUrl = getRequiredEnv('VITE_SIGNALR_URL').replace(/\/+$/, '')
    const hubPath = HUB_PATHS[this.kind]

    const builder = new HubConnectionBuilder()
      .withUrl(`${baseUrl}${hubPath}`, {
        accessTokenFactory: () => {
          if (this.kind === 'orders') {
            return useAuthStore.getState().token || ''
          }
          return ''
        }
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)

    return builder.build()
  }

  async connect(): Promise<void> {
    if (!this.connection) {
      this.connection = this.buildConnection()
    }

    if (this.connection.state === HubConnectionState.Disconnected) {
      await this.connection.start()
    }
  }

  async disconnect(): Promise<void> {
    if (!this.connection) return
    if (this.connection.state !== HubConnectionState.Disconnected) {
      await this.connection.stop()
    }
  }

  on(eventName: string, handler: (...args: any[]) => void): void {
    if (!this.connection) {
      this.connection = this.buildConnection()
    }
    this.connection.on(eventName, handler)
  }

  off(eventName: string, handler: (...args: any[]) => void): void {
    if (this.connection) {
      this.connection.off(eventName, handler)
    }
  }
}

export class MarketDataConnection extends BaseHubConnection {
  constructor() {
    super('market')
  }

  async joinSymbol(symbol: string): Promise<void> {
    if (!this.connection) throw new Error('Not connected')
    await this.connection.invoke('JoinSymbol', symbol)
  }

  async leaveSymbol(symbol: string): Promise<void> {
    if (!this.connection) throw new Error('Not connected')
    await this.connection.invoke('LeaveSymbol', symbol)
  }
}

export class OrderConnection extends BaseHubConnection {
  constructor() {
    super('orders')
  }
}
