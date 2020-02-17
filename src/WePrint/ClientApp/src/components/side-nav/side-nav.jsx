import React from 'react';
import { useRouteMatch } from 'react-router-dom';
import NavItem from './components/nav-item';
import './side-nav.scss';

// handleNavToggle={isNavBarOpen => this.handleNavToggle(isNavBarOpen)}
// isNavOpen={isNavOpen}
const SideNav = () => {
  const match = useRouteMatch();
  return (
    <nav className="side-nav">
      <NavItem to="/" active={match.path === '/'}>
        Dashboard
      </NavItem>
      <NavItem to="/devices" active={match.path.startsWith('/devices')}>
        Devices
      </NavItem>
      <NavItem to="/topics" active={match.path.startsWith('/topics')}>
        Topics
      </NavItem>
      <NavItem to="/find" active={match.path.startsWith('/find')}>
        Find a Job
      </NavItem>
      <NavItem to="/post" active={match.path.startsWith('/post')}>
        Post a Job
      </NavItem>
      <NavItem to="/help" active={match.path.startsWith('/help')}>
        Help
      </NavItem>
      <NavItem to="/about" active={match.path.startsWith('/about')}>
        About
      </NavItem>
    </nav>
  );
};

export default SideNav;
