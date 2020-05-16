import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import PropTypes from 'prop-types';

import { BodyCard, StatusView } from '../../components';
import SearchApi from '../../api/SearchApi';
import SearchItem from './components/search-item';

class Find extends Component {
  constructor(props) {
    super(props);
    this.state = { results: null, error: false };
  }

  componentDidMount() {
    this.onRouteChanged();
  }

  componentDidUpdate(prevProps) {
    const { location } = this.props;
    if (location !== prevProps.location) {
      this.onRouteChanged();
    }
  }

  onRouteChanged() {
    const { location } = this.props;
    const q = location.state.query;

    if (q === '') return;

    SearchApi.Search(q).subscribe(
      results => {
        this.setState({
          results,
          error: false,
        });
      },
      err => {
        this.setState({
          error: true,
        });
        console.error(err);
      }
    );
  }

  render() {
    const { results, error } = this.state;

    if (error) {
      return (
        <BodyCard>
          <StatusView text="Could not load search results" icon={['far', 'frown']} />
        </BodyCard>
      );
    }

    if (!results) {
      return (
        <BodyCard>
          <StatusView text="Loading..." icon="sync" spin />
        </BodyCard>
      );
    }

    if (!results.length) {
      return (
        <BodyCard>
          <h2>No Results Found</h2>
        </BodyCard>
      );
    }

    return (
      <BodyCard>
        {results.map(item => {
          return <SearchItem item={item} key={item.id} />;
        })}
      </BodyCard>
    );
  }
}

Find.propTypes = {
  location: PropTypes.objectOf(PropTypes.oneOfType([PropTypes.object, PropTypes.string]))
    .isRequired,
};

export default withRouter(Find);
