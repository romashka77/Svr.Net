import React from 'react';
import './claim-list-item.css';

const ClaimListItem = ({ claim/*, onAddedToCart */}) => {
  const { title, author, price, coverImage } = claim;
  return (
    <div className="claim-list-item">
      <div className="claim-cover">
        <img src={coverImage} alt="cover" />
      </div>
      <div className="claim-details">
        <span className="claim-title">{title}</span>
        <div className="claim-author">{author}</div>
        <div className="claim-price">${price}</div>
        <button
          //onClick={onAddedToCart}
          className="btn btn-info add-to-cart">
          Add to cart
        </button>
      </div>
    </div>
  );
};

export default ClaimListItem;
