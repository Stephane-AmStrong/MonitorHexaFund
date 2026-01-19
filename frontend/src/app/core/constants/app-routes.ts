export const APP_ROUTES = {
  DASHBOARD: 'dashboard',
  ALERTS: {
    ROOT: 'alerts',
    LIST: '',
  },
  CLIENTS: {
    ROOT: 'clients',
    LIST: '',
    DETAIL: 'login/:login',
  },
  CONNECTIONS: {
    ROOT: 'connections',
    LIST: '',
    DETAIL: ':id',
  },
  HOSTS: {
    ROOT: 'hosts',
    LIST: '',
    DETAIL: ':hostName',
    APPS_REDIRECT: ':hostName/apps',
    APP_BY_HOST: ':hostName/apps/:appName',
  },
  APPS: {
    ROOT: 'apps',
    LIST: '',
  },
  APPS_STATUS: {
    ROOT: 'app-statuses',
    LIST: '',
    DETAIL: ':id',
  },
  ROOT: '',
  ERRORS: {
    BAD_GATEWAY: 'bad-gateway',
    FORBIDDEN: 'forbidden',
    INTERNAL_SERVER_ERROR: 'internal-server-error',
    RATE_LIMIT: 'rate-limit',
    RESOURCE_NOT_FOUND: 'resource-not-found',
    ROOT: 'error',
    SERVICE_UNAVAILABLE: 'service-unavailable',
    UNKNOWN: 'unknown',
    UNAUTHORIZED: 'unauthorized',
  },
  NOT_FOUND: '**',
} as const;
