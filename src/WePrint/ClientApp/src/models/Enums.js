export const JobStatus = {
  PendingOpen: 'PendingOpen',
  BiddingOpen: 'BiddingOpen',
  BiddingClosed: 'BiddingClosed',
  BidSelected: 'BidSelected',
  PrintComplete: 'PrintComplete',
  Shipped: 'Shipped',
  Received: 'Received',
  Closed: 'Closed',
  Cancelled: 'Cancelled',
};

export const finishedJobStatuses = [JobStatus.Received, JobStatus.Closed, JobStatus.Cancelled];

export const PrinterType = {
  SLA: 'SLA',
  FDM: 'FDM',
  LaserCut: 'LaserCut',
};

export const MaterialType = {
  ABS: 'ABS',
  PLA: 'PLA',
  Resin: 'Resin',
  Polycarbonate: 'Polycarbonate',
  Flexible: 'Flexible',
};

export const MaterialColor = {
  Red: 'Red',
  Green: 'Green',
  Blue: 'Blue',
  Yellow: 'Yellow',
  Clear: 'Clear',
  Any: 'Any',
};

export const FinishType = {
  Sanding: 'Sanding',
  Varnish: 'Varnish',
  Priming: 'Priming',
  Painting: 'Painting',
  None: 'None',
};

export const PledgeStatus = {
  NotStarted: 'NotStarted',
  InProgress: 'InProgress',
  Shipped: 'Shipped',
  Finished: 'Finished',
  Canceled: 'Canceled',
};

export const USStates = [
  'AL',
  'AK',
  'AZ',
  'AR',
  'CA',
  'CO',
  'CT',
  'DE',
  'FL',
  'GA',
  'HI',
  'ID',
  'IL',
  'IN',
  'IA',
  'KS',
  'KY',
  'LA',
  'ME',
  'MD',
  'MA',
  'MI',
  'MN',
  'MS',
  'MO',
  'MT',
  'NE',
  'NV',
  'NH',
  'NJ',
  'NM',
  'NY',
  'NC',
  'ND',
  'OH',
  'OK',
  'OR',
  'PA',
  'RI',
  'SC',
  'SD',
  'TN',
  'TX',
  'UT',
  'VT',
  'VA',
  'WA',
  'WV',
  'WI',
  'WY',
];
