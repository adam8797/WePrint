import React from 'react';
import { BodyCard } from '../../components';
import JobApi from '../../api/JobApi';

// THIS IS A DEMO OF THE API
// As jobs are added/removed from this user, they'll populate the list here

class FindJob extends React.Component {
  constructor() {
    super();
    this.state = { myJobs: [] };
  }

  componentDidMount() {
    // When the component mounts, we start tracking jobs. We store the subscription becasue we MUST unsubscribe when we're done with it.
    this.subscription = JobApi.TrackMyJobs(1000).subscribe(myJobs => {
      this.setState({ myJobs });
    }, console.error);
  }

  componentWillUnmount() {
    // when we unmount, we unsubscribe.
    if (this.subscription) this.subscription.unsubscribe();
  }

  render() {
    const { myJobs } = this.state;
    return (
      <BodyCard>
        <h2>Find a Job</h2>
        <ul>
          {myJobs.map(job => (
            <li key={job.id}>
              {job.id}: {job.name} {job.status}
            </li>
          ))}
        </ul>
      </BodyCard>
    );
  }
}

export default FindJob;
