import { ConnectionResponse } from '../connections/models/connection-response';
import { AppStatusDetailedResponse } from './models/app-status-detailed-response';

export const MOCK_APP_STATUSES: AppStatusDetailedResponse[] = [
  {
    // Status 1 - Up
    id: 'STATUS-1',
    appId: 'APP-1',
    status: "Up",
    recordedAt: '2025-08-20T11:58:00.000Z',
    app: {
      id: 'APP-1',
      hostName: 'DTCODEVMCSDX04W',
      appName: 'DgxEgm',
      port: '31000',
      type: 'mas-app',
      cronStartTime: '30 07 * * 1',
      cronStopTime: '00 22 * * 1',
      version: '1.0.0',
      tags: ['md-trayport-egm-uat.mcs.gem.myengie.com:31000'],
      latestStatus: {
        id: 'STATUS-1',
        appId: 'APP-1',
        status: "Up",
        recordedAt: '2025-08-20T11:58:00.000Z',
      },
    },
  },
  {
    // Status 2 - Up
    id: 'STATUS-2',
    appId: 'APP-2',
    status: "Up",
    recordedAt: '2025-08-20T11:59:30.000Z',
    app: {
      id: 'APP-2',
      hostName: 'DTCODEVMCSGT01W',
      appName: 'Gateway',
      port: '5432',
      type: 'database-app',
      cronStartTime: '00 06 * * *',
      cronStopTime: '00 23 * * *',
      version: '2.1.0',
      tags: ['postgresql-prod.db.myengie.com:5432', 'primary'],
      latestStatus: {
        id: 'STATUS-2',
        appId: 'APP-2',
        status: "Up",
        recordedAt: '2025-08-20T11:59:30.000Z',
      },
    },
  },
  {
    // Status 3 - Down
    id: 'STATUS-3',
    appId: 'APP-3',
    status: "Down",
    recordedAt: '2025-08-20T10:15:00.000Z',
    app: {
      id: 'APP-3',
      hostName: 'DTCODEVMCSHP01W',
      appName: 'Horsepower-Orders',
      port: '80',
      type: 'api-gateway',
      cronStartTime: '00 00 * * *',
      cronStopTime: '23 59 * * *',
      version: '3.0.1',
      tags: ['api-gateway.myengie.com:80', 'api-gateway.myengie.com:443'],
      latestStatus: {
        id: 'STATUS-3',
        appId: 'APP-3',
        status: "Down",
        recordedAt: '2025-08-20T10:15:00.000Z',
      },
    },
  },
  {
    // Status 4 - Up
    id: 'STATUS-4',
    appId: 'APP-4',
    status: "Up",
    recordedAt: '2025-08-20T11:59:45.000Z',
    app: {
      id: 'APP-4',
      hostName: 'Redis Cache Cluster',
      appName: 'Horsepower-Trades',
      port: '6379',
      type: 'cache-app',
      cronStartTime: '00 00 * * *',
      cronStopTime: '23 59 * * *',
      version: '7.2.0',
      tags: ['redis-cache.myengie.com:6379', 'node-1'],
      latestStatus: {
        id: 'STATUS-4',
        appId: 'APP-4',
        status: "Up",
        recordedAt: '2025-08-20T11:59:45.000Z',
      },
    },
  },
  {
    // Status 5 - Up
    id: 'STATUS-5',
    appId: 'APP-5',
    status: "Up",
    recordedAt: '2025-08-20T11:57:00.000Z',
    app: {
      id: 'APP-5',
      hostName: 'DTCODEVMCSMELW',
      appName: 'Feedchecker-TGE',
      port: '5672',
      type: 'message-queue',
      cronStartTime: '00 05 * * *',
      cronStopTime: '23 55 * * *',
      version: '3.12.0',
      tags: ['rabbitmq.myengie.com:5672', 'rabbitmq-mgmt.myengie.com:15672'],
      latestStatus: {
        id: 'STATUS-5',
        appId: 'APP-5',
        status: "Up",
        recordedAt: '2025-08-20T11:57:00.000Z',
      },
    },
  },
];
