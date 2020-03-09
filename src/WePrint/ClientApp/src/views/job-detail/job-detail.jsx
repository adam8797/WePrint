import React from 'react';
import { useParams } from 'react-router-dom';

import { BodyCard } from '../../components';
import Job from './components/job';

function JobDetail() {
  const { jobId } = useParams();
  return (
    <BodyCard>
      <Job jobId={jobId} />
    </BodyCard>
  );
}

export default JobDetail;
