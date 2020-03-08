import React from 'react';
import PropTypes from 'prop-types';
import JobCard from './components/job-card';
import JobPlaceholder from '../../assets/images/job.png';

import './job-grid.scss';

function JobGrid(props) {
  const { jobs } = props;

  function getGridFooter() {
    if (!jobs.length) {
      return <div className="job-grid__content--empty">No Data To Display</div>;
    }
    return (
      <div className="job-grid__content-count">
        Showing <strong>{jobs.length}</strong> Result{jobs.length !== 1 && 's'}
      </div>
    );
  }

  return (
    <div className="job-grid">
      <div className="job-grid__content">
        {jobs.map(job => (
          <JobCard
            name={job.name}
            image={job.image}
            jobId={job.id}
            userName={job.userName}
            customerId={job.customerId}
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
      <div className="job-grid__footer">{getGridFooter()}</div>
    </div>
  );
}

JobGrid.propTypes = {
  jobs: PropTypes.arrayOf(
    PropTypes.shape({
      // required
      name: PropTypes.string.isRequired,
      id: PropTypes.string.isRequired,
      customerId: PropTypes.string.isRequired,
      userName: PropTypes.string.isRequired,
      // optional
      image: PropTypes.string,
      source: PropTypes.string,
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
