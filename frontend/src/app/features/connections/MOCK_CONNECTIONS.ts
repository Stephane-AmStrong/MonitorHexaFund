import { ConnectionResponse } from './models/connection-response';
import { ConnectionDetailedResponse } from './models/connection-detailed-response';

export const MOCK_CONNECTIONS: ConnectionDetailedResponse[] = [
 {
    // Connection 1
    id: "conn-001",
    clientGaia: "gaia-12345",
    appId: "app-001",
    apiVersion: "v2.0",
    machine: "SERVER-01",
    processId: "5432",
    establishedAt: "2026-01-20T10:30:00Z",
    client: {
      id: "client-001",
      login: "john.doe",
      gaia: "gaia-12345"
    },
    app: {
      id: "app-001",
      hostName: "prod-host-01",
      appName: "AuthService",
      port: "8443",
      type: "microservice",
      latestStatus: {
        id: 'STATUS-2',
        appId: 'APP-2',
        status: 'Up',
        recordedAt: '2025-08-20T11:59:30.000Z',
      },
      cronStartTime: "09:00",
      cronStopTime: "22:00",
      version: "2.5.1",
      tags: ["auth", "critical"]
    }
  },
  {
    // Connection 2
    id: 'CONN-2',
    clientGaia: 'CLIENT-2',
    appId: 'APP-2',
    apiVersion: '2.1.0',
    machine: 'WORKSTATION-002',
    processId: '5891',
    establishedAt: "2026-01-20T10:30:00Z",
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
    appId: 'APP-3',
    apiVersion: '3.0.1',
    machine: 'WORKSTATION-003',
    processId: '7234',
    establishedAt: "2026-01-20T10:30:00Z",
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
