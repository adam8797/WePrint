import React from 'react';
import PropTypes from 'prop-types';
import { useHistory } from 'react-router-dom';

import JobPlaceholder from '../../../assets/images/job.png';
import './job-card.scss';

function JobCard(props) {
  const { image, name, jobId, user, source, externalId, parts, printTime, prints } = props;
  const history = useHistory();

  return (
    // TODO: may want to revisit this to enable keyboard interaction of the page?
    // eslint-disable-next-line jsx-a11y/click-events-have-key-events
    <div
      className="job-card"
      onClick={() => {
        history.push(`/job/${jobId}`);
      }}
    >
      <img className="job-card__image" src={image} alt="Job" />
      <div className="job-card__body">
        <div className="job-card__detail">
          <div>
            <span>{name}</span>
            <span className="job-card__text--small">{user}</span>
          </div>
          <div>
            <span>{source}</span>
            <span className="job-card__text--small job-card__text--right">{externalId}</span>
          </div>
        </div>
        <div className="job-card__print">
          <div>
            <span className="job-card__text--medium">{parts}</span>
            <span className="job-card__text--small">Parts</span>
          </div>
          <div>
            <span className="job-card__text--medium">{printTime}</span>
            <span className="job-card__text--small">Print Time</span>
          </div>
          <div>
            <span className="job-card__text--medium">{prints}</span>
            <span className="job-card__text--small">Total Prints</span>
          </div>
        </div>
      </div>
    </div>
  );
}

JobCard.propTypes = {
  // required
  name: PropTypes.string.isRequired,
  jobId: PropTypes.string.isRequired,
  user: PropTypes.string.isRequired,
  source: PropTypes.string,
  // optional
  image: PropTypes.string,
  externalId: PropTypes.string,
  printTime: PropTypes.string,
  parts: PropTypes.string,
  prints: PropTypes.string,
};

JobCard.defaultProps = {
  image: JobPlaceholder,
  source: 'Custom',
  externalId: '',
  printTime: '-',
  parts: '-',
  prints: '-',
};

export default JobCard;
