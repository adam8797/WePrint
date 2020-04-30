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
  faSync,
  faTimes,
  faBuilding as faBuildingSolid,
} from '@fortawesome/free-solid-svg-icons';

import {
  faComments,
  faBuilding,
  faQuestionCircle as faQuestionCircleOutline,
  faFrown,
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
  faBuildingSolid,
  faQuestionCircleOutline,
  faTrash,
  faPen,
  faFrown,
  faSync,
  faTimes
);
