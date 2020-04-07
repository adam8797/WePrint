import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router-dom';

import './search-bar.scss';

class SearchBar extends Component {
  constructor(props) {
    super(props);
    this.state = {
      query: '',
      isButtonDisabled: true,
    };
  }

  submit = e => {
    const { history } = this.props;
    const { query } = this.state;
    history.push('/find', { query });
    e.preventDefault();
  };

  textChange = e => {
    this.setState({
      query: e.target.value,
      isButtonDisabled: !e.target.value.length,
    });
  };

  render() {
    const { query, isButtonDisabled } = this.state;
    return (
      <div className="search-bar">
        <input
          type="text"
          name="search"
          value={query}
          id="search"
          className="search-bar__input"
          placeholder="Search for makers or jobs"
          onChange={this.textChange}
        />
        <button
          className="search-bar__submit"
          onClick={this.submit}
          disabled={isButtonDisabled}
          type="submit"
        >
          GO
        </button>
      </div>
    );
  }
}

SearchBar.propTypes = {
  history: PropTypes.objectOf(
    PropTypes.oneOfType([PropTypes.string, PropTypes.number, PropTypes.object])
  ).isRequired,
};

export default withRouter(SearchBar);
