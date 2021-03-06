import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import './job-progress.scss';

export const ProgressStatus = {
  INACTIVE: 'inactive',
  ACTIVE: 'active',
  COMPLETE: 'complete',
};

function ProgressArrow({ name = '', status = ProgressStatus.INACTIVE }) {
  const className = classNames('progress-arrow', `progress-arrow--${status}`);
  return (
    <div className={className}>
      <div className="progress-arrow__name">{name}</div>
    </div>
  );
}
ProgressArrow.propTypes = {
  name: PropTypes.string,
  status: PropTypes.oneOf(Object.values(ProgressStatus)),
};

function checkProgressStatus(active, stateId) {
  if (active > stateId) {
    return ProgressStatus.COMPLETE;
  }
  if (active === stateId) {
    return ProgressStatus.ACTIVE;
  }
  return ProgressStatus.INACTIVE;
}

function ArrowProgressBar({ stages, active = 0 }) {
  return (
    <div className="arrow-progress-bar" style={{ '--num-arrows': stages.length }}>
      <ProgressArrow
        name=""
        status={active > 0 ? ProgressStatus.COMPLETE : ProgressStatus.ACTIVE}
      />
      {stages &&
        stages.map((state, i) => {
          return (
            <ProgressArrow
              key={state.name}
              name={state.name}
              status={checkProgressStatus(active, i)}
            />
          );
        })}
      <ProgressArrow name="" status={checkProgressStatus(active, stages.length - 1)} />
    </div>
  );
}

ArrowProgressBar.propTypes = {
  stages: PropTypes.arrayOf(
    PropTypes.shape({
      name: PropTypes.string,
    })
  ).isRequired,
  active: PropTypes.number,
};

export default ArrowProgressBar;
