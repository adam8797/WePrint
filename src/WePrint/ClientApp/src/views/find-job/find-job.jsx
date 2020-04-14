import React from 'react';
import { withRouter } from 'react-router-dom';
import { get } from 'lodash';

import { BodyCard } from '../../components';
import ToggleableDisplay from '../../components/toggleable-display/toggleable-display';
import JobApi from '../../api/JobApi';

class FindJob extends React.Component {
  constructor(props) {
    super(props);
    this.state = { jobResults: [] };
  }

  render() {
    // get the current query string from the location
    const q = get(this, 'props.location.state.query', '');

    // if the query string has changed...
    if (this.query !== q) {
      // we need to add remove the old subscription and add a new one
      JobApi.SearchJobs(q).subscribe(jobResults => {
        this.setState({
          jobResults,
        });
      }, console.error);
      this.query = q;
    }

    const { jobResults } = this.state;

    const jobs = jobResults.map(job => ({
      ...job,
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
