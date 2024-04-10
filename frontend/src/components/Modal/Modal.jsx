import CloseIcon from '@mui/icons-material/Close';

const Modal = ({ onClose, children }) => {
  return (
    <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-30 z-50">
      <div className="bg-white p-8 rounded-md max-w-[600px] w-full">
        <button className="absolute top-4 right-4 text-white" onClick={onClose}>
          <CloseIcon/>
        </button>
        {children}
      </div>
    </div>
  );
};

export default Modal;