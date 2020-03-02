import React from 'react';
import PropTypes from 'prop-types';
import JobCard from './components/job-card';

import './job-grid.scss';

function JobGrid(props) {
  const { jobs } = props;
  return (
    <div className="job-grid">
      {jobs.map(job => (
        <JobCard
          name={job.name}
          image={job.image}
          jobId={job.jobId}
          user={job.user}
          parts={job.parts}
          printTime={job.printTime}
          prints={job.prints}
          source={job.source}
          externalId={job.externalId}
          key={job.jobId}
        />
      ))}
    </div>
  );
}

JobGrid.propTypes = {
  jobs: PropTypes.arrayOf(PropTypes.objectOf(PropTypes.string)),
};

export default JobGrid;
