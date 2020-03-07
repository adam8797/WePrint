import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

import './search-bar.scss';

class SearchBar extends Component {
  constructor(props) {
    super(props);
    this.state = {
      query: '',
      isButtonDisabled: true,
    };
    this.submit = this.submit.bind(this);
    this.textChange = this.textChange.bind(this);
  }

  submit(e) {
    this.props.history.push('/find', { query: this.state.query });
    e.preventDefault();
  }

  textChange(e) {
    this.setState({
      query: e.target.value,
      isButtonDisabled: e.target.value.length > 0 ? false : true,
    });
  }

  render() {
    return (
      <div className="search-bar">
        <input
          type="text"
          name="search"
          value={this.state.query}
          id="search"
          className="search-bar__input"
          placeholder="Search for makers or jobs"
          onChange={this.textChange}
        />
        <button
          className="search-bar__submit"
          onClick={this.submit}
          disabled={this.state.isButtonDisabled}
        >
          GO
        </button>
      </div>
    );
  }
}

export default withRouter(SearchBar);
