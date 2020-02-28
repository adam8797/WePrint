import React from 'react';
import { useParams } from 'react-router-dom';

import { BodyCard } from '../../components';
import Job from './components/job';

function JobDetail() {
  let { jobId } = useParams();
  return (
    <BodyCard>
      <Job jobId={jobId}></Job>
    </BodyCard>
  );
}

export default JobDetail;
