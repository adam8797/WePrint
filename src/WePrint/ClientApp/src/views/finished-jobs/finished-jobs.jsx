import React, { Component } from 'react';
import JobApi from '../../api/JobApi';

import BodyCard from '../../components/body-card/body-card';
import ToggleableDisplay from '../../components/toggleable-display/toggleable-display';
import { finishedJobStatuses } from '../../models/Enums';

class FinishedJobs extends Component {
  constructor(props) {
    super(props);
    this.state = {
      jobs: [],
    };
  }

  componentDidMount() {
    this.subscription = JobApi.TrackMyJobs(1000).subscribe(jobs => {
      this.setState({ jobs: Array.isArray(jobs) ? jobs : [] });
    }, console.error);
  }

  componentWillUnmount() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  render() {
    const { jobs } = this.state;

    const displayJobs = jobs
      .filter(job => {
        return finishedJobStatuses.includes(job.status);
      })
      .map(job => ({
        ...job,
        link: `/job/${job.id}`,
      }));

    return (
      <BodyCard>
        <ToggleableDisplay title="My Finished Jobs" data={displayJobs} />
      </BodyCard>
    );
  }
}

export default FinishedJobs;
