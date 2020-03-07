import React, { Component } from 'react';
import JobApi from '../../api/JobApi';

import BodyCard from '../../components/body-card/body-card';
import ToggleableDisplay from '../../components/toggleable-display/toggleable-display';

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
        return job.status === 6 || job.status === 7 || job.status === 8;
      })
      .map(job => ({
        ...job,
        user: job.customerId.replace('ApplicationUsers-', ''),
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
