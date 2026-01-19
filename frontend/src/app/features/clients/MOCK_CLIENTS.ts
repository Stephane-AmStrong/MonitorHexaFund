import { ClientDetailedResponse } from './models/client-detailed-response';

export const MOCK_CLIENTS: ClientDetailedResponse[] = [
  {
    id: '550e8400-e29b-41d4-a716-446655440000',
    login: 'admin@production.com',
    gaia: 'WZ6655',
    connections: [
      {
        id: 'conn-001',
        clientGaia: '550e8400-e29b-41d4-a716-446655440000',
        clientLogin: 'admin@production.com',
        appId: 'srv-001',
        apiVersion: '1.0.0',
        machine: 'prod-app-01',
        processId: '4521',
      },
      {
        id: 'conn-002',
        clientGaia: '550e8400-e29b-41d4-a716-446655440000',
        clientLogin: 'admin@production.com',
        appId: 'srv-002',
        apiVersion: '1.0.0',
        machine: 'prod-worker-01',
        processId: '3840',
      },
    ],
  },
  {
    id: '550e8400-e29b-41d4-a716-446655440001',
    login: 'staging@company.com',
    gaia: 'BS6655',
    connections: [
      {
        id: 'conn-003',
        clientGaia: '550e8400-e29b-41d4-a716-446655440001',
        clientLogin: 'staging@company.com',
        appId: 'srv-003',
        apiVersion: '1.0.0-beta',
        machine: 'staging-app-01',
        processId: '2156',
      },
    ],
  },
  {
    id: '550e8400-e29b-41d4-a716-446655440002',
    login: 'dev@company.com',
    gaia: 'JD7744',
    connections: [
      {
        id: 'conn-004',
        clientGaia: '550e8400-e29b-41d4-a716-446655440002',
        clientLogin: 'dev@company.com',
        appId: 'srv-004',
        apiVersion: '1.1.0-dev',
        machine: 'dev-pc-01',
        processId: '5890',
      },
      {
        id: 'conn-005',
        clientGaia: '550e8400-e29b-41d4-a716-446655440002',
        clientLogin: 'dev@company.com',
        appId: 'srv-005',
        apiVersion: '1.1.0-dev',
        machine: 'dev-pc-01',
        processId: '6234',
      },
      {
        id: 'conn-006',
        clientGaia: '550e8400-e29b-41d4-a716-446655440002',
        clientLogin: 'dev@company.com',
        appId: 'srv-006',
        apiVersion: '1.0.5',
        machine: 'dev-service-01',
        processId: '1024',
      },
    ],
  },
  {
    id: '550e8400-e29b-41d4-a716-446655440003',
    login: 'backup@company.com',
    gaia: 'MK5533',
    connections: [
      {
        id: 'conn-007',
        clientGaia: '550e8400-e29b-41d4-a716-446655440003',
        clientLogin: 'backup@company.com',
        appId: 'srv-007',
        apiVersion: '0.9.0',
        machine: 'backup-app-01',
        processId: '7821',
      },
    ],
  }
];