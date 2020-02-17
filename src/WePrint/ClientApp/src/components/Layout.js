import React from 'react';
import PropTypes from 'prop-types';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';

export default function Layout({ children }) {
  return (
    <div>
      <NavMenu />
      <Container>{children}</Container>
    </div>
  );
}

Layout.propTypes = {
  children: PropTypes.node.isRequired,
};

Layout.displayName = Layout.name;
