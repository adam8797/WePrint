import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import  ProjectApi  from '../../api/ProjectApi';
import { BodyCard, SectionTitle } from '../../components';

class Recieved extends Component {
  constructor(props) {
    super(props);
    this.state = { 
      subscription: null,
      finished: false,
    };
  }

  componentDidMount() {
    const { match } = this.props;
    const { projId, pledgeId } = match.params;
    this.setState({
      subscription: ProjectApi.pledgesFor(projId).complete(pledgeId).subscribe(this.completeResult.bind(this)),
    });
  }

  completeResult() {
    this.setState({
      finished: true,
    });
  }

  render() {
    const { subscription, finished } = this.state;

    if (subscription === null) {
      return <div>Please wait...</div>;
    }

    let content = <div>
      <SectionTitle title="Working..." />
      <p>Please wait...</p>
      <FontAwesomeIcon icon="spinner" spin />
    </div>;

    if(finished) {
      content = <div>
        <SectionTitle title="Success!" />
        <p>Pledge Recieved! Feel free to close this page.</p>
      </div>;
    }

    return (<BodyCard className="homepage">
      {content}
    </BodyCard>);
  }
}


Recieved.propTypes = {
  match: PropTypes.objectOf(
    PropTypes.oneOfType([PropTypes.string, PropTypes.bool, PropTypes.object])
  ).isRequired,
};

export default withRouter(Recieved);
