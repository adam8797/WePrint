import { library } from '@fortawesome/fontawesome-svg-core';
import { fab } from '@fortawesome/free-brands-svg-icons';
import {
  faCheckSquare,
  faCoffee,
  faTachometerAlt,
  faMicrochip,
  faSearchDollar,
  faFileInvoiceDollar,
  faReceipt,
  faQuestionCircle,
  faInfoCircle,
  faBars,
  faTh,
  faArrowLeft,
  faTrash,
  faPen,
} from '@fortawesome/free-solid-svg-icons';

import {
  faComments,
  faBuilding,
  faQuestionCircle as faQuestionCircleOutline,
} from '@fortawesome/free-regular-svg-icons';

// fonts should be added to the library here once so we're
// not including more than we need or explicitly loading them everywhere
library.add(
  fab,
  faCheckSquare,
  faCoffee,
  faTachometerAlt,
  faMicrochip,
  faSearchDollar,
  faFileInvoiceDollar,
  faReceipt,
  faQuestionCircle,
  faInfoCircle,
  faBars,
  faTh,
  faArrowLeft,
  faComments,
  faBuilding,
  faQuestionCircleOutline,
  faTrash,
  faPen
);
