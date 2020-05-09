import React, { Component } from 'react';

import OrgApi from '../../api/OrganizationApi';
import { ToggleableDisplay, BodyCard, StatusView, CardTypes } from '../../components';

class Organizations extends Component {
  constructor(props) {
    super(props);
    this.state = {
      organizations: null,
      error: false,
    };

    this.columns = [
      {
        Header: 'Name',
        accessor: 'name',
      },
      {
        Header: 'Location',
        accessor: 'location',
      },
      {
        Header: 'Projects',
        accessor: 'projectCount',
      },
    ];
  }

  componentDidMount() {
    OrgApi.getAll().subscribe(
      organizations => {
        const orgsWithLinks = organizations.map(org => {
          return {
            ...org,
            link: `/organization/${org.id}`,
            location: `${org.address.city}, ${org.address.state}`,
            projectCount: `${org.projects.length} project${org.projects.length === 1 ? '' : 's'}`,
          };
        });
        this.setState({ organizations: orgsWithLinks });
      },
      err => {
        console.error(err);
        this.setState({ error: true });
      }
    );
  }

  render() {
    const { organizations, error } = this.state;
    if (error) {
      return (
        <BodyCard>
          <StatusView text="Could not load organizations" icon={['far', 'frown']} />
        </BodyCard>
      );
    }
    if (organizations === null) {
      return (
        <BodyCard>
          <StatusView text="Loading organizations..." icon="sync" spin />
        </BodyCard>
      );
    }

    return (
      <BodyCard>
        <ToggleableDisplay
          title="Organizations"
          data={organizations}
          cardType={CardTypes.ORGANIZATION}
          columns={this.columns}
        />
      </BodyCard>
    );
  }
}

export default Organizations;
