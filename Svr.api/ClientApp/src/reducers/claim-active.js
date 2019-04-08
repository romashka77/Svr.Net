import { CLAIM_SELECTED } from '../constants/claims';

export default function (state = null, action) {
  switch (action.type) {
    case CLAIM_SELECTED:
      return action.payload;
    default:
      return state;
  }
}