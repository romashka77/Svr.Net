// Первая заглавная, остальные строчные
const textFirstUpperCase = (value) => {
  if (value.length > 0) {
    return `${value.charAt(0).toUpperCase()}${value.substr(1, value.length - 1).toLowerCase()}`;
  } return value;
};
export default textFirstUpperCase;
