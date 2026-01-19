import { ConnectionResponse } from './models/connection-response';
import { ConnectionDetailedResponse } from './models/connection-detailed-response';

export const MOCK_CONNECTIONS: ConnectionDetailedResponse[] = [
 {
    // Connection 1
    id: 'CONN-1',
    clientGaia: 'CLIENT-1',
    clientLogin: 'john.doe@engie.com',
    appId: 'APP-1',
    apiVersion: '1.0.0',
    machine: 'WORKSTATION-001',
    processId: '4532',
    client: {
      id: 'CLIENT-1',
      login: 'john.doe@engie.com',
      gaia: 'GAIA-12345',
    },
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
        status: 'Up',
        recordedAt: '2025-08-20T11:58:00.000Z',
      },
    },
  },
  {
    // Connection 2
    id: 'CONN-2',
    clientGaia: 'CLIENT-2',
    clientLogin: 'marie.smith@engie.com',
    appId: 'APP-2',
    apiVersion: '2.1.0',
    machine: 'WORKSTATION-002',
    processId: '5891',
    client: {
      id: 'CLIENT-2',
      login: 'marie.smith@engie.com',
      gaia: 'GAIA-67890',
    },
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
        status: 'Up',
        recordedAt: '2025-08-20T11:59:30.000Z',
      },
    },
  },
  {
    // Connection 3
    id: 'CONN-3',
    clientGaia: 'CLIENT-3',
    clientLogin: 'pierre.martin@engie.com',
    appId: 'APP-3',
    apiVersion: '3.0.1',
    machine: 'WORKSTATION-003',
    processId: '7234',
    client: {
      id: 'CLIENT-3',
      login: 'pierre.martin@engie.com',
      gaia: 'GAIA-11223',
    },
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
        status: 'Down',
        recordedAt: '2025-08-20T10:15:00.000Z',
      },
    },
  }
];
