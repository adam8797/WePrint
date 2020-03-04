import React from 'react';
import PropTypes from 'prop-types';
import JobCard from './components/job-card';
import JobPlaceholder from '../../assets/images/job.png';

import './job-grid.scss';

function JobGrid(props) {
  const { jobs } = props;
  return (
    <div className="job-grid">
      {jobs.map(job => (
        <JobCard
          name={job.name}
          image={job.image}
          jobId={job.id}
          user={job.user}
          parts={job.parts}
          printTime={job.printTime}
          prints={job.prints}
          source={job.source}
          externalId={job.externalId}
          status={job.status}
          key={job.id}
        />
      ))}
    </div>
  );
}

JobGrid.propTypes = {
  jobs: PropTypes.arrayOf(
    PropTypes.shape({
      // required
      name: PropTypes.string.isRequired,
      jobId: PropTypes.string.isRequired,
      user: PropTypes.string.isRequired,
      source: PropTypes.string.isRequired,
      // optional
      image: PropTypes.string,
      externalId: PropTypes.string,
      printTime: PropTypes.string,
      parts: PropTypes.string,
      prints: PropTypes.string,
    })
  ),
};

JobGrid.defaultProps = {
  jobs: [
    {
      image: JobPlaceholder,
      source: 'Custom',
      externalId: '',
      printTime: '-',
      parts: '-',
      prints: '-',
    },
  ],
};

export default JobGrid;
