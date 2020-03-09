import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { SideNav, Header } from '../../components';
import './app-layout.scss';

class AppLayout extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isNavOpen: true,
    };
  }

  handleNavToggle(isNavBarOpen) {
    this.setState({ isNavOpen: isNavBarOpen });
  }

  render() {
    const { children } = this.props;
    const { isNavOpen } = this.state;
    return (
      <div id="appLayout" className="app-layout">
        <div className="app-layout__content-wrap">
          <div className="app-layout__header">
            {/* <GlobalHeader
              // currentUser={user}
              logoHeight={28}
              // logoIcon={`o-in-app-logo-${activeTheme}`}
              // userMenuOptions={this.getUserMenuOptions()}
            /> */}
            <Header />
          </div>
          <div className="app-layout__sidebar">
            <SideNav
              handleNavToggle={isNavBarOpen => this.handleNavToggle(isNavBarOpen)}
              isNavOpen={isNavOpen}
            />
          </div>
          <div className="app-layout__content-main">
            <main id="app-layout" className="app-layout__content">
              {children}
            </main>
          </div>
        </div>
      </div>
    );
  }
}

AppLayout.propTypes = {
  children: PropTypes.node.isRequired,
};

export default AppLayout;
