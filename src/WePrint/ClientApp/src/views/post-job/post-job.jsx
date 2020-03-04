import React, { Component } from 'react';
import { BodyCard, Button } from '../../components';
import ArrowProgressBar from './components/job-progress';

class PostJob extends Component {
  constructor(props) {
    super(props);
    this.state = {
      currentState: 0,
    };
  }

  setJobType(type) {
    this.setState(prevState => ({
      job: {
        ...prevState.job,
        type,
      },
      currentState: 1,
    }));
  }

  advanceState() {
    this.setState(prevState => ({ currentState: prevState.currentState + 1 }));
  }

  reverseState() {
    this.setState(prevState => ({ currentState: prevState.currentState - 1 }));
  }

  render() {
    const { currentState } = this.state;
    const states = [
      {
        name: 'Job Type',
      },
      {
        name: 'Basic Info',
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
        {currentState === 0 && (
          <BodyCard centered>
            <h2>Select Type of Job</h2>
            <Button type={Button.Type.PRIMARY} onClick={() => this.setJobType('FDM')}>
              FDM
            </Button>
            <Button type={Button.Type.PRIMARY} onClick={() => this.setJobType('SLA')}>
              SLA
            </Button>
            <Button type={Button.Type.PRIMARY} onClick={() => this.setJobType('Laser')}>
              Laser
            </Button>
          </BodyCard>
        )}
        {currentState === 1 && (
          <BodyCard centered>
            <h2>Basic Info</h2>
            <p>Form Stuff Here</p>
            <Button type={Button.Type.DANGER} onClick={() => this.reverseState()}>
              Back
            </Button>
            <Button type={Button.Type.SUCCESS} onClick={() => this.advanceState()}>
              Next
            </Button>
          </BodyCard>
        )}
        {currentState === 2 && (
          <BodyCard centered>
            <h2>Upload Files</h2>
            <Button type={Button.Type.DANGER} onClick={() => this.reverseState()}>
              Back
            </Button>
            <Button type={Button.Type.SUCCESS} onClick={() => this.advanceState()}>
              Next
            </Button>
          </BodyCard>
        )}
        {currentState === 3 && (
          <BodyCard centered>
            <h2>Pre-Process</h2>
            <Button type={Button.Type.DANGER} onClick={() => this.reverseState()}>
              Back
            </Button>
            <Button type={Button.Type.SUCCESS} onClick={() => this.advanceState()}>
              Done
            </Button>
          </BodyCard>
        )}
      </>
    );
  }
}

export default PostJob;
