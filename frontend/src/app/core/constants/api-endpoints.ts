export const API_ENDPOINTS = {
  ALERTS: {
    ROOT: 'alerts',
    BY_ID: (id: string) => `${API_ENDPOINTS.ALERTS.ROOT}/${id}`,
    LIVE: () => `${API_ENDPOINTS.ALERTS.ROOT}/live`
  },
  DASHBOARD: {
    ROOT: 'dashboard',
  },
  CLIENTS: {
    ROOT: 'clients',
    BY_ID: (id: string) => `${API_ENDPOINTS.CLIENTS.ROOT}/${id}`,
    BY_LOGIN: (login: string) => `${API_ENDPOINTS.CLIENTS.ROOT}/login/${login}`,
    LIVE: () => `${API_ENDPOINTS.CLIENTS.ROOT}/live`
  },
  CONNECTIONS: {
    ROOT: 'connections',
    BY_ID: (id: string) => `${API_ENDPOINTS.CONNECTIONS.ROOT}/${id}`,
    LIVE: () => `${API_ENDPOINTS.CONNECTIONS.ROOT}/live`
  },
  HOSTS: {
    ROOT: 'hosts',
    BY_ID: (id: string) => `${API_ENDPOINTS.HOSTS.ROOT}/by-id/${id}`,
    BY_NAME: (name: string) => `${API_ENDPOINTS.HOSTS.ROOT}/${name}`,
    WITH_APPS: () => `${API_ENDPOINTS.HOSTS.ROOT}/with-apps`,
    APP_BY_HOST: (hostName: string, appName: string) => `${API_ENDPOINTS.HOSTS.ROOT}/${hostName}/apps/${appName}`,
    LIVE: () => `${API_ENDPOINTS.HOSTS.ROOT}/live`
  },
  APPS: {
    ROOT: 'apps',
    BY_ID: (id: string) => `${API_ENDPOINTS.APPS.ROOT}/${id}`,
    LIVE: () => `${API_ENDPOINTS.APPS.ROOT}/live`
  },
  APP_STATUSES: {
    ROOT: 'app-statuses',
    BY_ID: (id: string) => `${API_ENDPOINTS.APP_STATUSES.ROOT}/${id}`,
    LIVE: () => `${API_ENDPOINTS.APP_STATUSES.ROOT}/live`
  },
} as const;
