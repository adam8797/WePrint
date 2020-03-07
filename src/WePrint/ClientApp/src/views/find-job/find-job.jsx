import React from 'react';
import { BodyCard } from '../../components';
import { withRouter } from 'react-router-dom';
import { get } from 'lodash';
import ToggleableDisplay from '../../components/toggleable-display/toggleable-display';
import JobApi from '../../api/JobApi';

class FindJob extends React.Component {
  constructor(props) {
    super(props);
    this.state = { jobResults: [] };
  }

  componentWillUnmount() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  render() {
    // get the current query string from the location
    const q = get(this, 'props.location.state.query', '');

    // if the query string has changed...
    if (this.query !== q) {
      //we need to add remove the old subscription and add a new one
      if (this.subscription) this.subscription.unsubscribe();
      this.subscription = JobApi.SearchJobs(q).subscribe(jobResults => {
        this.setState({
          jobResults,
        });
      }, console.error);
      this.query = q;
    }

    const { jobResults } = this.state;

    const jobs = jobResults.map(job => ({
      ...job,
      user: job.customerId.replace('ApplicationUsers-', ''),
      link: `/job/${job.id}`,
    }));

    return (
      <BodyCard>
        <ToggleableDisplay title="Find a Job" data={jobs} />
      </BodyCard>
    );
  }
}

export default withRouter(FindJob);
