import React from 'react';
import { useParams } from 'react-router-dom';

import { BodyCard, Job } from '../../components';


function JobDetail() {
  let { jobId } = useParams();
  return (
    <BodyCard>
      <Job jobId={jobId}></Job>
    </BodyCard>
  );
}

export default JobDetail;