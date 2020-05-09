import React from 'react';
import { get } from 'lodash';
import PropTypes from 'prop-types';
import { BodyCard } from '../../components';
import SearchApi from '../../api/SearchApi';
import { withRouter } from 'react-router-dom';

import SearchItem from './components/search-item'

class Find extends React.Component {
  constructor(props) {
    super(props);
    this.state = { jobResults: [] };
  }

  static propTypes = {
      location: PropTypes.object.isRequired
  }

  componentDidUpdate(prevProps) {
      if (this.props.location !== prevProps.location) {
          this.onRouteChanged();
      }
  }

  onRouteChanged() {
    const q = this.props.location.state.query;
    console.log("Searching for: " + q);

    if (q === "")
      return;

    SearchApi
      .Search(q)
      .subscribe(results => {
          this.setState({
            results
          });
        },
        console.error);
  }

  render() {

    const results = this.state.results;

    if (results === undefined)
      return (
        <BodyCard>
          <h2>No Results Found</h2>
        </BodyCard>
      );

    return (
      <BodyCard>
        {this.state.results.map((item, index) => {
            return (<SearchItem item={item} />);
        })}
      </BodyCard>
    );
  }
}

export default withRouter(Find);
