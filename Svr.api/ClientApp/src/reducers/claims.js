import { REQUEST_CLAIMS, RECEIVE_CLAIMS } from '../constants/claims';

export default function (state = null, action) {
  switch (action.type) {
    case REQUEST_CLAIMS:
      return action.payload;
    case RECEIVE_CLAIMS:
      return action.payload;
    default:
      return state;
  }
}