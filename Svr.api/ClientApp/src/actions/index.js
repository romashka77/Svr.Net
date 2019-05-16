import claimsLoadedType from  '../action-types/claim-types';

const claimsLoaded = (newClaims) => {
  return {
    type: claimsLoadedType,
    payload: newClaims
  };
};

export {
  claimsLoaded
};
