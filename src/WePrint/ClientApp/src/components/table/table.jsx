import React from 'react';
import PropTypes from 'prop-types';
import { useHistory } from 'react-router-dom';
import { useTable } from 'react-table';
import SectionTitle from '../section-title/section-title';
import './table.scss';

function Table({ title, columns, data, actions }) {
  const { getTableProps, getTableBodyProps, headerGroups, rows, prepareRow } = useTable({
    columns,
    data,
  });
  const history = useHistory();

  function clickAction(row) {
    if (row.original.link) {
      history.push(row.original.link);
    }
  }

  function getTableFooter() {
    if (!data.length) {
      return <div className="table__content--empty">No Data To Display</div>;
    }
    return (
      <div className="table__content-count">
        Showing <strong>{data.length}</strong> Results
      </div>
    );
  }

  return (
    <div className="table">
      {title && <SectionTitle title={title} actions={actions} />}
      <table {...getTableProps()} className="table__content">
        <thead>
          {headerGroups.map(headerGroup => (
            <tr {...headerGroup.getHeaderGroupProps()}>
              {headerGroup.headers.map(column => (
                <th {...column.getHeaderProps()}>{column.render('Header')}</th>
              ))}
            </tr>
          ))}
        </thead>
        <tbody {...getTableBodyProps()}>
          {rows.map(row => {
            prepareRow(row);
            let clickable = '';
            let onclick = () => {};
            if (row.original.link) {
              clickable = 'table__row--clickable';
              onclick = () => clickAction(row);
            }
            return (
              <tr {...row.getRowProps()} className={`table__row ${clickable}`} onClick={onclick}>
                {row.cells.map(cell => {
                  return <td {...cell.getCellProps()}>{cell.render('Cell')}</td>;
                })}
              </tr>
            );
          })}
        </tbody>
      </table>
      <div className="table__footer">{getTableFooter()}</div>
    </div>
  );
}

Table.propTypes = {
  title: PropTypes.string,
  columns: PropTypes.arrayOf(PropTypes.objectOf(PropTypes.string)).isRequired,
  data: PropTypes.arrayOf(PropTypes.objectOf(PropTypes.string)).isRequired,
  actions: PropTypes.arrayOf(
    PropTypes.shape({ text: PropTypes.string, key: PropTypes.string, action: PropTypes.func })
  ),
};

export default Table;
