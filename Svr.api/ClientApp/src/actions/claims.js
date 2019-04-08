import { CLAIM_SELECTED } from '../constants/claims';

export const select = (claim) => {
  return {
    type: CLAIM_SELECTED,
    payload: claim
  };
}