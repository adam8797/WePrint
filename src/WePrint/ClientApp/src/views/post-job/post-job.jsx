import React, { Component } from 'react';
import { BodyCard } from '../../components';
import ArrowProgressBar from './components/job-progress';

class PostJob extends Component {
  constructor(props) {
    super(props);
    this.state = {
      currentState: 0,
    };
  }

  render() {
    const { currentState } = this.state;
    const states = [
      {
        name: 'Basic Information',
      },
      {
        name: 'Upload Files',
      },
      {
        name: 'Pre-Process',
      },
    ];
    return (
      <>
        <ArrowProgressBar states={states} active={currentState} />
        <BodyCard centered>
          <h2>Post a Job</h2>
        </BodyCard>
      </>
    );
  }
}

export default PostJob;
