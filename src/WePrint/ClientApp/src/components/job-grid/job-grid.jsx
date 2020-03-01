import React from 'react';
import PropTypes from 'prop-types';
import JobCard from './components/job-card';

import './job-grid.scss';

function JobGrid({ jobs }) {
  return (
    <div className="job-grid">
      <JobCard
        name="DnD Minis"
        jobId="1234"
        user="Emily"
        parts="4"
        printTime="4h"
        prints="4"
        source="Thingiverse"
        externalId="12345678"
      />
      <JobCard
        name="DnD Minis"
        jobId="1234"
        user="Emily"
        image="http://placekitten.com/250/250"
        parts="4"
        printTime="4h"
        prints="4"
        source="Custom"
      />
      <JobCard
        name="DnD Minis"
        jobId="1234"
        user="Emily"
        image="http://placekitten.com/500/500"
        parts="4"
        printTime="4h"
        prints="4"
        source="Custom"
      />
      <JobCard
        name="DnD Minis"
        jobId="1234"
        user="Emily"
        image="http://placekitten.com/900"
        parts="4"
        printTime="4h"
        prints="4"
        source="Custom"
      />
    </div>
  );
}

JobGrid.propTypes = {
  jobs: PropTypes.arrayOf(PropTypes.objectOf(PropTypes.string)),
};

export default JobGrid;
