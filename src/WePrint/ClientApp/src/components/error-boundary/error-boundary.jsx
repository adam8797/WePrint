import React, { Component } from 'react';
import PropTypes from 'prop-types';
import BodyCard from '../body-card/body-card';
import StatusView from '../status-view/status-view';

class ErrorBoundary extends Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError() {
    // Update state so the next render will show the fallback UI.
    return { hasError: true };
  }

  componentDidCatch(error, errorInfo) {
    // You can also log the error to an error reporting service
    // logErrorToMyService(error, errorInfo);
    console.error(error, errorInfo);
  }

  render() {
    const { hasError } = this.state;
    const { children } = this.props;

    if (hasError) {
      // You can render any custom fallback UI
      return (
        <BodyCard>
          <StatusView
            text="A fatal error occurred preventing this page from loading. Please try again in a few minutes. Contact us if the problem persists."
            icon={['far', 'frown']}
          />
        </BodyCard>
      );
    }

    return children;
  }
}

ErrorBoundary.propTypes = {
  children: PropTypes.element.isRequired,
};

export default ErrorBoundary;
