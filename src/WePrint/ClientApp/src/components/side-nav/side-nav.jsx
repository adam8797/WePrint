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
      {/* <NavItem to="/" active={match.path === '/'} icon="tachometer-alt">
        Dashboard
      </NavItem>
      <NavItem to="/devices" active={match.path.startsWith('/devices')} icon="microchip">
        Devices
      </NavItem>
      <NavItem to="/find" active={match.path.startsWith('/find')} icon="search-dollar">
        Find a Job
      </NavItem>
      <NavItem to="/post" active={match.path.startsWith('/post')} icon="file-invoice-dollar">
        Post a Job
      </NavItem>
      <NavItem to="/finished" active={match.path.startsWith('/finished')} icon="receipt">
        Finished Jobs
      </NavItem>
      */}
      <NavItem to="/" active={match.path === '/'} icon="home">
        Home
      </NavItem>
      <NavItem to="/projects" active={match.path.startsWith('/projects')} icon="hands-helping">
        Projects
      </NavItem>
      <NavItem to="/create-project" active={match.path.startsWith('/create-project')} icon="plus">
        New Project
      </NavItem>
      <NavItem to="/my-org" active={match.path.startsWith('/my-org')} icon="id-badge">
        My Organization
      </NavItem>
      <NavItem to="/organizations" active={match.path.startsWith('/organizations')} icon="building">
        Organizations
      </NavItem>
      <NavItem to="/help" active={match.path.startsWith('/help')} icon="question-circle">
        Help
      </NavItem>
    </nav>
  );
};

export default SideNav;
