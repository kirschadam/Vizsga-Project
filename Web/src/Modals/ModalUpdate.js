import React from "react";

export function ModalUpdate({ onClosed, Submitted, children}) {
    return (
        <div className="modal d-block">
            <div className="modal-dialog">
                <div className="modal-content">
                    <form onSubmit={Submitted}>
                        <div className="modal-header">
                            <h5 className="modal-title">Update Product</h5>
                            <button type="button" className="close" onClick={onClosed}>
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                    
                        <div className="modal-body">{children}</div>

                        <div className="modal-footer">
                            <button type="submit" className="btn btn-primary">Confirm</button>
                            <button type="button" className="btn btn-secondary" onClick={onClosed}>Cancel</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}